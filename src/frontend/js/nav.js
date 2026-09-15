/*
 * Navegação pública e autenticada, compartilhada por todas as páginas —
 * antes cada uma tinha seu próprio HTML de menu copiado e colado, e isso
 * já tinha divergido de verdade (Home com "Blog/Login" vs. Blog com
 * "Início/Entrar"; só 2 de 6 páginas autenticadas destacavam a página
 * atual no menu). Agora existe um único lugar que define o menu.
 */
const PAGINAS_AUTENTICADAS = [
  { chave: 'dashboard', label: 'Dashboard', href: '../Página do Usuario/Usuario.html' },
  { chave: 'perfil', label: 'Perfil', href: '../Perfil Usuario/PerfilU.html' },
  { chave: 'metas', label: 'Metas', href: '../Metas Financeiras/Metas.html' },
  { chave: 'calculadoras', label: 'Calculadoras', href: '../Calculadoras/Calculadoras.html' },
  { chave: 'blog', label: 'Blog', href: '../Conteudo/Conteudo.HTML' },
];

function navPublicaHtml() {
  return `
    <li class="nav-item"><a class="nav-link" href="../Home/Home.html">Início</a></li>
    <li class="nav-item"><a class="nav-link" href="../Home/Home.html#beneficios">Funcionalidades</a></li>
    <li class="nav-item"><a class="nav-link" href="../Home/Home.html#como-funciona">Como funciona</a></li>
    <li class="nav-item"><a class="nav-link" href="../Conteudo/Conteudo.HTML">Blog</a></li>
    <li class="nav-item"><a class="nav-link" href="../Login/Login.HTML">Entrar</a></li>
    <li class="nav-item"><a class="btn btn-success ms-md-2 mt-2 mt-md-0" href="../Cadastro/Cadastro.html">Começar agora</a></li>
  `;
}

function navAutenticadaHtml(paginaAtiva) {
  const itens = PAGINAS_AUTENTICADAS.map(({ chave, label, href }) => {
    const ativa = chave === paginaAtiva;
    const classe = ativa ? 'nav-link active' : 'nav-link';
    const ariaCurrent = ativa ? ' aria-current="page"' : '';
    return `<li class="nav-item"><a class="${classe}"${ariaCurrent} href="${href}">${label}</a></li>`;
  }).join('\n    ');

  return `
    ${itens}
    <li class="nav-item"><a class="nav-link" href="../Home/Home.html" id="logout-link">Sair</a></li>
  `;
}

/**
 * Preenche o <ul> de navegação de acordo com a sessão atual e liga o
 * botão de logout quando autenticado.
 * @param {string} elementId - id do <ul> de navegação
 * @param {string} [paginaAtiva] - chave da página atual (ver PAGINAS_AUTENTICADAS),
 *   usada só quando o usuário está logado, para destacar onde ele está.
 */
function montarNav(elementId, paginaAtiva) {
  const nav = document.getElementById(elementId);
  if (!nav) return;

  const logado = typeof estaAutenticado === 'function' && estaAutenticado();
  nav.innerHTML = logado ? navAutenticadaHtml(paginaAtiva) : navPublicaHtml();

  if (logado) {
    document.getElementById('logout-link')?.addEventListener('click', (event) => {
      event.preventDefault();
      logout();
      window.location.reload();
    });
  }
}
