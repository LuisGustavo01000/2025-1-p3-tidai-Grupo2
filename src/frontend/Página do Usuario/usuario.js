/*
 * Dashboard real do CashWise: carrega transações e metas do usuário
 * autenticado via API, calcula saldo/gastos do mês no próprio navegador
 * (não existe endpoint de agregação no backend ainda — ver observações
 * passadas ao usuário) e permite registrar uma nova transação.
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();

  const statSaldo = document.getElementById('stat-saldo');
  const statGastosMes = document.getElementById('stat-gastos-mes');
  const statMetas = document.getElementById('stat-metas');
  const tbody = document.getElementById('transacoes-tbody');
  const erroBox = document.getElementById('transacoes-erro');
  const form = document.getElementById('nova-transacao-form');
  const logoutLink = document.getElementById('logout-link');

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
      tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Nenhuma transação registrada ainda.</td></tr>';
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
          </tr>
        `;
      })
      .join('');
  }

  function calcularResumo(transacoes) {
    const agora = new Date();
    let saldo = 0;
    let gastosMes = 0;

    for (const t of transacoes) {
      const valor = Number(t.valor) || 0;
      const ehReceita = t.tipo?.toLowerCase() === 'receita';

      saldo += ehReceita ? valor : -valor;

      const dataTransacao = new Date(t.data);
      const mesmoMes =
        dataTransacao.getMonth() === agora.getMonth() &&
        dataTransacao.getFullYear() === agora.getFullYear();

      if (!ehReceita && mesmoMes) {
        gastosMes += valor;
      }
    }

    statSaldo.textContent = formatarMoeda(saldo);
    statSaldo.classList.toggle('text-success', saldo >= 0);
    statSaldo.classList.toggle('text-danger', saldo < 0);
    statGastosMes.textContent = formatarMoeda(gastosMes);
  }

  async function carregarTransacoes() {
    try {
      const response = await apiFetch('/Transacao');

      if (!response.ok) {
        mostrarErro('Não foi possível carregar suas transações agora.');
        tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">—</td></tr>';
        return;
      }

      const transacoes = await response.json();
      renderizarTransacoes(transacoes);
      calcularResumo(transacoes);
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    }
  }

  async function carregarMetas() {
    try {
      const response = await apiFetch('/MetaFinanceira');
      if (!response.ok) return;

      const metas = await response.json();
      statMetas.textContent = metas.length;
    } catch {
      // Silencioso: metas são um dado secundário nesta tela.
    }
  }

  form?.addEventListener('submit', async (event) => {
    event.preventDefault();
    erroBox.classList.add('d-none');

    const descricao = form.descricao.value.trim();
    const valor = parseFloat(form.valor.value);
    const tipo = form.tipo.value;

    if (!descricao || Number.isNaN(valor) || valor <= 0) {
      mostrarErro('Preencha a descrição e um valor válido maior que zero.');
      return;
    }

    const botao = form.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/Transacao', {
        method: 'POST',
        body: JSON.stringify({ descricao, valor, tipo }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível registrar a transação.');
        mostrarErro(mensagem);
        return;
      }

      form.reset();
      await carregarTransacoes();
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    } finally {
      botao.disabled = false;
    }
  });

  logoutLink?.addEventListener('click', (event) => {
    event.preventDefault();
    logout();
    window.location.href = '../Login/Login.HTML';
  });

  carregarTransacoes();
  carregarMetas();
});
