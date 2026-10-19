/*
 * Sessão do usuário no frontend do CashWise.
 *
 * O token fica em localStorage. Isso NÃO é o ideal de segurança (um XSS na
 * página consegue ler localStorage e roubar o token) — o ideal seria um
 * cookie httpOnly setado pelo próprio backend. Optamos por localStorage
 * nesta fase porque o frontend é HTML/JS estático sem servidor próprio
 * para gerenciar cookies, e porque ainda não há dados reais de usuários em
 * produção. Reavaliar quando o projeto for para produção de verdade
 * (ver backlog CW2-020/CW2-031).
 */
const CASHWISE_TOKEN_KEY = "cashwise_token";
const CASHWISE_USUARIO_KEY = "cashwise_usuario";

function salvarSessao({ token, usuarioId, nome, email, expiraEmUtc }) {
  localStorage.setItem(CASHWISE_TOKEN_KEY, token);
  localStorage.setItem(
    CASHWISE_USUARIO_KEY,
    JSON.stringify({ usuarioId, nome, email, expiraEmUtc })
  );
}

function getToken() {
  return localStorage.getItem(CASHWISE_TOKEN_KEY);
}

function getUsuarioLogado() {
  const bruto = localStorage.getItem(CASHWISE_USUARIO_KEY);
  return bruto ? JSON.parse(bruto) : null;
}

function estaAutenticado() {
  const usuario = getUsuarioLogado();
  if (!getToken() || !usuario?.expiraEmUtc) {
    return false;
  }
  return new Date(usuario.expiraEmUtc) > new Date();
}

function logout() {
  localStorage.removeItem(CASHWISE_TOKEN_KEY);
  localStorage.removeItem(CASHWISE_USUARIO_KEY);
}

/**
 * Usar no topo de páginas que exigem login: manda de volta para o Login
 * se não houver sessão válida.
 */
function exigirLogin(caminhoLogin = "../Login/Login.HTML") {
  if (!estaAutenticado()) {
    window.location.href = caminhoLogin;
  }
}
