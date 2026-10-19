/*
 * Home.js — monta a navegação (pública ou autenticada, conforme sessão) e
 * puxa os 3 artigos mais recentes do Blog da API real, em vez dos 3 cards
 * fixos e sem link que existiam antes.
 */
document.addEventListener('DOMContentLoaded', () => {
  montarNav('nav-links');
  carregarPostsDoBlog();
});

function escapeHtmlHome(texto) {
  const div = document.createElement('div');
  div.textContent = texto;
  return div.innerHTML;
}

async function carregarPostsDoBlog() {
  const grid = document.getElementById('home-blog-grid');
  const erroBox = document.getElementById('home-blog-erro');

  try {
    const response = await apiFetch('/Conteudo');

    if (!response.ok) {
      grid.innerHTML = '';
      erroBox.textContent = 'Não foi possível carregar o blog agora — veja todos os artigos na página do Blog.';
      erroBox.classList.remove('d-none');
      return;
    }

    const posts = await response.json();

    if (posts.length === 0) {
      grid.innerHTML = '<div class="col-12 text-muted small">Ainda não há artigos publicados pela comunidade — os primeiros conteúdos aparecem aqui assim que forem criados.</div>';
      return;
    }

    grid.innerHTML = posts
      .slice(0, 3)
      .map(
        (post) => `
          <div class="col-md-6 col-lg-4">
            <a href="../Conteudo/Conteudo.HTML" class="text-decoration-none">
              <article class="blog-post p-4 bg-white rounded shadow-sm h-100 text-start">
                <span class="badge bg-secondary mb-2">${escapeHtmlHome(post.tipo)}</span>
                <h3 class="h5 text-success">${escapeHtmlHome(post.titulo)}</h3>
                <p class="mb-0 text-dark">${escapeHtmlHome(post.descricao.slice(0, 110))}${post.descricao.length > 110 ? '…' : ''}</p>
              </article>
            </a>
          </div>
        `
      )
      .join('');
  } catch {
    grid.innerHTML = '';
    erroBox.textContent = 'Não foi possível conectar ao servidor do CashWise — veja todos os artigos na página do Blog.';
    erroBox.classList.remove('d-none');
  }
}
