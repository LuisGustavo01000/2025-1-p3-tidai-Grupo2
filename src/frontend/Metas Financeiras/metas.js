/*
 * Metas Financeiras — o backend (GET/POST/PUT/DELETE /api/MetaFinanceira)
 * já existia e já tinha testes automatizados; esta é a primeira tela que
 * o utiliza. Sem "progresso" fake: o modelo não guarda quanto já foi
 * poupado, só nome/valor-alvo/prazo/status, então só mostramos o que é
 * real (e os dias restantes, que dá pra calcular de verdade a partir do prazo).
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();
  montarNav('nav-links', 'metas');

  const grid = document.getElementById('metas-grid');
  const vazio = document.getElementById('metas-vazio');
  const erroBox = document.getElementById('metas-erro');
  const template = document.getElementById('meta-card-template');

  const novaMetaForm = document.getElementById('nova-meta-form');
  const novaMetaErro = document.getElementById('nova-meta-erro');
  const modalEl = document.getElementById('modalNovaMeta');

  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  function classeStatus(status) {
    const s = status.toLowerCase();
    if (s.includes('conclu')) return 'status-concluida';
    if (s.includes('quase')) return 'status-quase-la';
    return 'status-andamento';
  }

  function descricaoPrazo(prazoIso) {
    const prazo = new Date(prazoIso);
    const hoje = new Date();
    hoje.setHours(0, 0, 0, 0);
    prazo.setHours(0, 0, 0, 0);

    const dias = Math.round((prazo - hoje) / (1000 * 60 * 60 * 24));
    const prazoFormatado = prazo.toLocaleDateString('pt-BR');

    if (dias < 0) {
      return { texto: `Prazo era ${prazoFormatado} — vencido há ${Math.abs(dias)} dia(s)`, vencido: true };
    }
    if (dias === 0) {
      return { texto: `Prazo é hoje (${prazoFormatado})`, vencido: false };
    }
    return { texto: `${prazoFormatado} — faltam ${dias} dia(s)`, vencido: false };
  }

  function mostrarErro(mensagem) {
    erroBox.textContent = mensagem;
    erroBox.classList.remove('d-none');
  }

  function criarCard(meta) {
    const fragmento = template.content.cloneNode(true);
    const prazo = descricaoPrazo(meta.prazo);

    fragmento.querySelector('.meta-nome').textContent = meta.nome;
    fragmento.querySelector('.meta-valor').textContent = formatarMoeda(meta.valor);

    const badge = fragmento.querySelector('.meta-status-badge');
    badge.textContent = meta.status;
    badge.classList.add(classeStatus(meta.status));

    const prazoEl = fragmento.querySelector('.meta-prazo');
    prazoEl.textContent = prazo.texto;
    if (prazo.vencido) prazoEl.classList.add('prazo-vencido');

    fragmento.querySelector('.meta-excluir').addEventListener('click', () => excluirMeta(meta.id));

    return fragmento;
  }

  function renderizarMetas(metas) {
    grid.innerHTML = '';

    if (metas.length === 0) {
      vazio.classList.remove('d-none');
      return;
    }

    vazio.classList.add('d-none');
    metas.forEach((meta) => grid.appendChild(criarCard(meta)));
  }

  async function carregarMetas() {
    try {
      const response = await apiFetch('/MetaFinanceira');

      if (!response.ok) {
        mostrarErro('Não foi possível carregar suas metas agora.');
        grid.innerHTML = '';
        return;
      }

      renderizarMetas(await response.json());
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
      grid.innerHTML = '';
    }
  }

  async function excluirMeta(id) {
    if (!confirm('Tem certeza que quer excluir esta meta?')) return;

    try {
      const response = await apiFetch(`/MetaFinanceira/${id}`, { method: 'DELETE' });

      if (!response.ok) {
        mostrarErro('Não foi possível excluir esta meta agora.');
        return;
      }

      await carregarMetas();
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    }
  }

  novaMetaForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    novaMetaErro.classList.add('d-none');

    const nome = novaMetaForm.nome.value.trim();
    const valor = parseFloat(novaMetaForm.valor.value);
    const prazo = novaMetaForm.prazo.value;
    const status = novaMetaForm.status.value;

    if (!nome || Number.isNaN(valor) || valor <= 0 || !prazo) {
      novaMetaErro.textContent = 'Preencha todos os campos corretamente.';
      novaMetaErro.classList.remove('d-none');
      return;
    }

    const botao = novaMetaForm.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/MetaFinanceira', {
        method: 'POST',
        body: JSON.stringify({ nome, valor, prazo, status }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível salvar a meta.');
        novaMetaErro.textContent = mensagem;
        novaMetaErro.classList.remove('d-none');
        return;
      }

      novaMetaForm.reset();
      bootstrap.Modal.getOrCreateInstance(modalEl).hide();
      await carregarMetas();
    } catch {
      novaMetaErro.textContent = 'Não foi possível conectar ao servidor do CashWise.';
      novaMetaErro.classList.remove('d-none');
    } finally {
      botao.disabled = false;
    }
  });

  carregarMetas();
});
