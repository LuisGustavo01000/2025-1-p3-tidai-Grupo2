/*
 * Conteudo.js — a seção de curadoria estática (carrossel + grade) continua
 * fixa em HTML porque é conteúdo real e útil (links externos). O que este
 * arquivo faz é ligar a seção "Conteúdo da Comunidade" à API real
 * (GET/POST/DELETE /api/Conteudo), que já existia no backend mas nunca
 * tinha sido usada por nenhuma tela.
 */
document.addEventListener('DOMContentLoaded', () => {
  const grid = document.getElementById('comunidade-grid');
  const vazio = document.getElementById('comunidade-vazio');
  const erroBox = document.getElementById('comunidade-erro');
  const template = document.getElementById('post-card-template');

  const btnPublicar = document.getElementById('btn-publicar');
  const avisoLogin = document.getElementById('comunidade-login-aviso');
  const navLinks = document.getElementById('nav-links');

  const publicarForm = document.getElementById('publicar-form');
  const publicarErro = document.getElementById('publicar-erro');
  const modalEl = document.getElementById('modalPublicar');

  const logado = typeof estaAutenticado === 'function' && estaAutenticado();
  const usuarioAtual = typeof getUsuarioLogado === 'function' ? getUsuarioLogado() : null;

  /* === navegação conforme sessão =============================== */
  // Mesma função usada pela Home — garante que as duas páginas públicas
  // nunca mais divirjam ("Blog/Login" vs. "Início/Entrar").
  montarNav('nav-links', 'blog');

  if (logado) {
    btnPublicar.classList.remove('d-none');
    avisoLogin.classList.add('d-none');
  }

  function formatarData(isoString) {
    return new Date(isoString).toLocaleDateString('pt-BR');
  }

  function escapeHtml(texto) {
    const div = document.createElement('div');
    div.textContent = texto;
    return div.innerHTML;
  }

  function mostrarErro(mensagem) {
    erroBox.textContent = mensagem;
    erroBox.classList.remove('d-none');
  }

  function criarCard(post) {
    const fragmento = template.content.cloneNode(true);

    fragmento.querySelector('.post-tipo').textContent = post.tipo;
    fragmento.querySelector('.post-nivel').textContent = post.nivel;
    fragmento.querySelector('.post-titulo').textContent = post.titulo;
    fragmento.querySelector('.post-descricao').textContent = post.descricao;
    fragmento.querySelector('.post-autor').textContent = `por ${post.autorNome}`;
    fragmento.querySelector('.post-data').textContent = formatarData(post.dataPublicacao);

    if (logado && usuarioAtual && post.autorNome === usuarioAtual.nome) {
      const btnExcluir = fragmento.querySelector('.post-excluir');
      btnExcluir.classList.remove('d-none');
      btnExcluir.addEventListener('click', () => excluirPost(post.id));
    }

    return fragmento;
  }

  function renderizarPosts(posts) {
    grid.innerHTML = '';

    if (posts.length === 0) {
      vazio.classList.remove('d-none');
      return;
    }

    vazio.classList.add('d-none');
    posts.forEach((post) => grid.appendChild(criarCard(post)));
  }

  async function carregarPosts() {
    try {
      const response = await apiFetch('/Conteudo');

      if (!response.ok) {
        mostrarErro('Não foi possível carregar o conteúdo da comunidade agora.');
        grid.innerHTML = '';
        return;
      }

      renderizarPosts(await response.json());
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
      grid.innerHTML = '';
    }
  }

  async function excluirPost(id) {
    if (!confirm('Tem certeza que quer excluir este conteúdo?')) return;

    try {
      const response = await apiFetch(`/Conteudo/${id}`, { method: 'DELETE' });
      if (!response.ok) {
        mostrarErro('Não foi possível excluir este conteúdo agora.');
        return;
      }
      await carregarPosts();
    } catch {
      mostrarErro('Não foi possível conectar ao servidor do CashWise.');
    }
  }

  publicarForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    publicarErro.classList.add('d-none');

    const titulo = publicarForm.titulo.value.trim();
    const descricao = publicarForm.descricao.value.trim();
    const tipo = publicarForm.tipo.value;
    const nivel = publicarForm.nivel.value;

    if (!titulo || !descricao) {
      publicarErro.textContent = 'Preencha título e conteúdo.';
      publicarErro.classList.remove('d-none');
      return;
    }

    const botao = publicarForm.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/Conteudo', {
        method: 'POST',
        body: JSON.stringify({ titulo, descricao, tipo, nivel }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível publicar o conteúdo.');
        publicarErro.textContent = mensagem;
        publicarErro.classList.remove('d-none');
        return;
      }

      publicarForm.reset();
      bootstrap.Modal.getOrCreateInstance(modalEl).hide();
      await carregarPosts();
    } catch {
      publicarErro.textContent = 'Não foi possível conectar ao servidor do CashWise.';
      publicarErro.classList.remove('d-none');
    } finally {
      botao.disabled = false;
    }
  });

  carregarPosts();
});
