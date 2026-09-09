/*
 * Dashboard real do CashWise: transações e resumo vêm da API. O relatório
 * (gráficos) é calculado no navegador a partir da lista de transações já
 * carregada para a tabela — sem endpoint de agregação dedicado ainda,
 * suficiente para o volume de dados de um usuário nesta fase do produto.
 *
 * Cores dos gráficos vêm da paleta categórica validada (contraste e
 * distinção sob daltonismo) usada no resto do projeto; Receita/Despesa
 * usa verde/vermelho para bater com o resto da UI (text-success/text-danger
 * já usados nos cards e na tabela).
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();
  montarNav('nav-links', 'dashboard');

  const statSaldo = document.getElementById('stat-saldo');
  const statGastosMes = document.getElementById('stat-gastos-mes');
  const statMetas = document.getElementById('stat-metas');
  const tbody = document.getElementById('transacoes-tbody');
  const erroBox = document.getElementById('transacoes-erro');
  const form = document.getElementById('nova-transacao-form');

  const filtroDe = document.getElementById('filtro-de');
  const filtroAte = document.getElementById('filtro-ate');
  const relatorioVazio = document.getElementById('relatorio-vazio');
  const relatorioGraficos = document.getElementById('relatorio-graficos');

  const CORES_CATEGORIA = {
    Moradia: '#2a78d6',
    'Alimentação': '#eb6834',
    Transporte: '#1baf7a',
    Lazer: '#eda100',
    'Saúde': '#e87ba4',
    'Salário': '#008300',
    'Educação': '#4a3aa7',
    Outros: '#e34948',
  };
  const COR_RECEITA = '#146b48';
  const COR_DESPESA = '#c0392b';

  let transacoesCache = [];
  let graficoMensal = null;
  let graficoCategorias = null;

  function formatarMoeda(valor) {
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  function formatarData(isoString) {
    return new Date(isoString).toLocaleDateString('pt-BR');
  }

  function mostrarErro(mensagem) {
    erroBox.textContent = mensagem;
    erroBox.classList.remove('d-none');
  }

  function escapeHtml(texto) {
    const div = document.createElement('div');
    div.textContent = texto;
    return div.innerHTML;
  }

  function renderizarTransacoes(transacoes) {
    if (transacoes.length === 0) {
      tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Nenhuma transação registrada ainda.</td></tr>';
      return;
    }

    tbody.innerHTML = transacoes
      .map((t) => {
        const ehReceita = t.tipo?.toLowerCase() === 'receita';
        const classeValor = ehReceita ? 'text-success' : 'text-danger';
        const sinal = ehReceita ? '+' : '-';

        return `
          <tr>
            <td>${formatarData(t.data)}</td>
            <td class="d-none d-md-table-cell">${escapeHtml(t.descricao)}</td>
            <td class="${classeValor} fw-bold">${sinal}${formatarMoeda(Math.abs(t.valor))}</td>
            <td class="d-none d-sm-table-cell">${escapeHtml(t.tipo)}</td>
            <td class="d-none d-md-table-cell">${escapeHtml(t.categoria || 'Outros')}</td>
          </tr>
        `;
      })
      .join('');
  }

  function renderizarResumo(resumo) {
    statSaldo.textContent = formatarMoeda(resumo.saldoTotal);
    statSaldo.classList.toggle('text-success', resumo.saldoTotal >= 0);
    statSaldo.classList.toggle('text-danger', resumo.saldoTotal < 0);
    statGastosMes.textContent = formatarMoeda(resumo.gastosMes);
    statMetas.textContent = resumo.metasAtivas;
  }

  /* === Relatório (gráficos) ===================================== */

  function transacoesNoFiltro() {
    const de = filtroDe.value ? new Date(`${filtroDe.value}T00:00:00`) : null;
    const ate = filtroAte.value ? new Date(`${filtroAte.value}T23:59:59`) : null;

    return transacoesCache.filter((t) => {
      const data = new Date(t.data);
      if (de && data < de) return false;
      if (ate && data > ate) return false;
      return true;
    });
  }

  function agruparPorMes(transacoes) {
    const porMes = new Map();

    for (const t of transacoes) {
      const data = new Date(t.data);
      const chave = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}`;

      if (!porMes.has(chave)) {
        porMes.set(chave, { receitas: 0, despesas: 0 });
      }

      const grupo = porMes.get(chave);
      const ehReceita = t.tipo?.toLowerCase() === 'receita';
      if (ehReceita) grupo.receitas += t.valor;
      else grupo.despesas += t.valor;
    }

    const chaves = [...porMes.keys()].sort();
    const rotulos = chaves.map((chave) => {
      const [ano, mes] = chave.split('-');
      return new Date(`${ano}-${mes}-01T00:00:00`).toLocaleDateString('pt-BR', { month: 'short', year: '2-digit' });
    });

    return {
      rotulos,
      receitas: chaves.map((c) => porMes.get(c).receitas),
      despesas: chaves.map((c) => porMes.get(c).despesas),
    };
  }

  function agruparDespesasPorCategoria(transacoes) {
    const porCategoria = new Map();

    for (const t of transacoes) {
      if (t.tipo?.toLowerCase() !== 'despesa') continue;
      const categoria = t.categoria || 'Outros';
      porCategoria.set(categoria, (porCategoria.get(categoria) || 0) + t.valor);
    }

    const categorias = [...porCategoria.keys()];
    return {
      categorias,
      valores: categorias.map((c) => porCategoria.get(c)),
      cores: categorias.map((c) => CORES_CATEGORIA[c] || CORES_CATEGORIA.Outros),
    };
  }

  function atualizarGraficoMensal(dados) {
    const ctx = document.getElementById('grafico-mensal');
    if (graficoMensal) graficoMensal.destroy();

    graficoMensal = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: dados.rotulos,
        datasets: [
          { label: 'Receitas', data: dados.receitas, backgroundColor: COR_RECEITA, borderRadius: 4 },
          { label: 'Despesas', data: dados.despesas, backgroundColor: COR_DESPESA, borderRadius: 4 },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom' },
          tooltip: {
            callbacks: {
              label: (ctx) => `${ctx.dataset.label}: ${formatarMoeda(ctx.parsed.y)}`,
            },
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: { callback: (valor) => formatarMoeda(valor) },
            grid: { color: '#e1e0d9' },
          },
          x: { grid: { display: false } },
        },
      },
    });
  }

  function atualizarGraficoCategorias(dados) {
    const ctx = document.getElementById('grafico-categorias');
    if (graficoCategorias) graficoCategorias.destroy();

    if (dados.categorias.length === 0) {
      graficoCategorias = null;
      ctx.getContext('2d').clearRect(0, 0, ctx.width, ctx.height);
      return;
    }

    graficoCategorias = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: dados.categorias,
        datasets: [{ data: dados.valores, backgroundColor: dados.cores, borderWidth: 2, borderColor: '#fff' }],
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom' },
          tooltip: {
            callbacks: {
              label: (ctx) => `${ctx.label}: ${formatarMoeda(ctx.parsed)}`,
            },
          },
        },
      },
    });
  }

  function atualizarRelatorio() {
    const filtradas = transacoesNoFiltro();

    if (filtradas.length === 0) {
      relatorioVazio.classList.remove('d-none');
      relatorioGraficos.classList.add('d-none');
      return;
    }

    relatorioVazio.classList.add('d-none');
    relatorioGraficos.classList.remove('d-none');

    atualizarGraficoMensal(agruparPorMes(filtradas));
    atualizarGraficoCategorias(agruparDespesasPorCategoria(filtradas));
  }

  document.querySelectorAll('[data-preset]').forEach((botao) => {
    botao.addEventListener('click', () => {
      const hoje = new Date();
      const formatar = (d) => d.toISOString().slice(0, 10);

      if (botao.dataset.preset === '30') {
        const inicio = new Date(hoje);
        inicio.setDate(inicio.getDate() - 30);
        filtroDe.value = formatar(inicio);
        filtroAte.value = formatar(hoje);
      } else if (botao.dataset.preset === 'mes') {
        filtroDe.value = formatar(new Date(hoje.getFullYear(), hoje.getMonth(), 1));
        filtroAte.value = formatar(hoje);
      } else {
        filtroDe.value = '';
        filtroAte.value = '';
      }

      atualizarRelatorio();
    });
  });

  filtroDe.addEventListener('change', atualizarRelatorio);
  filtroAte.addEventListener('change', atualizarRelatorio);

  /* === Carregamento inicial ====================================== */

  async function carregarTransacoes() {
    try {
      const response = await apiFetch('/Transacao');

      if (!response.ok) {
        mostrarErro('Não foi possível carregar suas transações agora.');
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">—</td></tr>';
        return;
      }

      transacoesCache = await response.json();
      renderizarTransacoes(transacoesCache);
      atualizarRelatorio();
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    }
  }

  async function carregarResumo() {
    try {
      const response = await apiFetch('/Dashboard/resumo');
      if (!response.ok) return;

      renderizarResumo(await response.json());
    } catch {
      // Silencioso: os cards ficam no valor inicial (R$ 0,00) se a API falhar.
    }
  }

  form?.addEventListener('submit', async (event) => {
    event.preventDefault();
    erroBox.classList.add('d-none');

    const descricao = form.descricao.value.trim();
    const valor = parseFloat(form.valor.value);
    const tipo = form.tipo.value;
    const categoria = form.categoria.value;

    if (!descricao || Number.isNaN(valor) || valor <= 0) {
      mostrarErro('Preencha a descrição e um valor válido maior que zero.');
      return;
    }

    const botao = form.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/Transacao', {
        method: 'POST',
        body: JSON.stringify({ descricao, valor, tipo, categoria }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível registrar a transação.');
        mostrarErro(mensagem);
        return;
      }

      form.reset();
      await Promise.all([carregarTransacoes(), carregarResumo()]);
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    } finally {
      botao.disabled = false;
    }
  });

  carregarTransacoes();
  carregarResumo();
});
