/*
 * Calculadoras do CashWise: três ferramentas puramente client-side (não
 * persistem nada) — não precisam de endpoint novo no backend, só a
 * calculadora de Meta lê dados reais (GET /MetaFinanceira) pra pré-preencher.
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();
  montarNav('nav-links', 'calculadoras');

  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  /* ===== Calculadora de Meta ===================================== */
  (function calculadoraMeta() {
    const selectMeta = document.getElementById('meta-existente');
    const inputValor = document.getElementById('meta-calc-valor');
    const inputAtual = document.getElementById('meta-calc-atual');
    const inputPrazo = document.getElementById('meta-calc-prazo');
    const form = document.getElementById('form-meta');
    const resultado = document.getElementById('meta-resultado');
    const spanValorMensal = document.getElementById('meta-valor-mensal');
    const spanDetalhe = document.getElementById('meta-detalhe');

    let metas = [];

    async function carregarMetas() {
      try {
        const response = await apiFetch('/MetaFinanceira');
        if (!response.ok) return;

        metas = await response.json();
        metas.forEach((meta) => {
          const option = document.createElement('option');
          option.value = meta.id;
          option.textContent = `${meta.nome} — ${formatarMoeda(meta.valor)}`;
          selectMeta.appendChild(option);
        });
      } catch {
        // select fica só com a opção manual se a API falhar
      }
    }

    selectMeta.addEventListener('change', () => {
      const meta = metas.find((m) => String(m.id) === selectMeta.value);
      if (!meta) return;

      inputValor.value = meta.valor;
      inputPrazo.value = meta.prazo.slice(0, 10);
    });

    form.addEventListener('submit', (event) => {
      event.preventDefault();

      const valorAlvo = parseFloat(inputValor.value);
      const jaGuardado = parseFloat(inputAtual.value) || 0;
      const prazo = new Date(`${inputPrazo.value}T00:00:00`);
      const hoje = new Date();

      const faltam = Math.max(valorAlvo - jaGuardado, 0);
      const diasRestantes = Math.max(Math.ceil((prazo - hoje) / (1000 * 60 * 60 * 24)), 1);
      const mesesRestantes = Math.max(Math.ceil(diasRestantes / 30), 1);
      const valorMensal = faltam / mesesRestantes;

      spanValorMensal.textContent = formatarMoeda(valorMensal);
      spanDetalhe.textContent = faltam === 0
        ? 'Você já atingiu o valor da meta!'
        : `Faltam ${formatarMoeda(faltam)} em aproximadamente ${mesesRestantes} ${mesesRestantes === 1 ? 'mês' : 'meses'}.`;
      resultado.classList.remove('d-none');
    });

    carregarMetas();
  })();

  /* ===== Orçamento 50/30/20 ======================================= */
  (function calculadoraOrcamento() {
    const form = document.getElementById('form-orcamento');
    const erro = document.getElementById('orc-erro');
    const resultado = document.getElementById('orc-resultado');

    form.addEventListener('submit', (event) => {
      event.preventDefault();
      erro.classList.add('d-none');

      const renda = parseFloat(document.getElementById('orc-renda').value);
      const pNecessidades = parseFloat(document.getElementById('orc-necessidades').value) || 0;
      const pDesejos = parseFloat(document.getElementById('orc-desejos').value) || 0;
      const pPoupanca = parseFloat(document.getElementById('orc-poupanca').value) || 0;

      const soma = pNecessidades + pDesejos + pPoupanca;
      if (Math.round(soma) !== 100) {
        erro.textContent = `Os percentuais precisam somar 100% (hoje somam ${soma}%).`;
        erro.classList.remove('d-none');
        resultado.classList.add('d-none');
        return;
      }

      document.getElementById('orc-valor-necessidades').textContent = formatarMoeda(renda * pNecessidades / 100);
      document.getElementById('orc-valor-desejos').textContent = formatarMoeda(renda * pDesejos / 100);
      document.getElementById('orc-valor-poupanca').textContent = formatarMoeda(renda * pPoupanca / 100);
      resultado.classList.remove('d-none');
    });
  })();

  /* ===== Juros compostos =========================================== */
  (function calculadoraJuros() {
    const form = document.getElementById('form-juros');
    const resultado = document.getElementById('juros-resultado');
    let grafico = null;

    form.addEventListener('submit', (event) => {
      event.preventDefault();

      const inicial = parseFloat(document.getElementById('juros-inicial').value) || 0;
      const aporte = parseFloat(document.getElementById('juros-aporte').value) || 0;
      const taxaMensal = (parseFloat(document.getElementById('juros-taxa').value) || 0) / 100;
      const periodo = parseInt(document.getElementById('juros-periodo').value, 10) || 0;

      const evolucao = [inicial];
      let saldo = inicial;
      for (let mes = 1; mes <= periodo; mes++) {
        saldo = saldo * (1 + taxaMensal) + aporte;
        evolucao.push(saldo);
      }

      const totalInvestido = inicial + aporte * periodo;
      document.getElementById('juros-total-investido').textContent = formatarMoeda(totalInvestido);
      document.getElementById('juros-valor-final').textContent = formatarMoeda(saldo);
      resultado.classList.remove('d-none');

      const ctx = document.getElementById('grafico-juros');
      if (grafico) grafico.destroy();
      grafico = new Chart(ctx, {
        type: 'line',
        data: {
          labels: evolucao.map((_, i) => `Mês ${i}`),
          datasets: [{
            label: 'Saldo projetado',
            data: evolucao,
            borderColor: '#146b48',
            backgroundColor: 'rgba(20, 107, 72, 0.12)',
            fill: true,
            tension: 0.25,
            pointRadius: 0,
          }],
        },
        options: {
          responsive: true,
          plugins: {
            legend: { display: false },
            tooltip: { callbacks: { label: (ctx) => formatarMoeda(ctx.parsed.y) } },
          },
          scales: {
            y: { ticks: { callback: (v) => formatarMoeda(v) }, grid: { color: '#e1e0d9' } },
            x: { grid: { display: false }, ticks: { maxTicksLimit: 8 } },
          },
        },
      });
    });
  })();
});
