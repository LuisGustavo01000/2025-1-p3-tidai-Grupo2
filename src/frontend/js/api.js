/*
 * Configuração central da API do CashWise.
 * Nenhum outro arquivo deve escrever a URL da API "na mão" —
 * sempre importar/usar API_BASE_URL e apiFetch a partir daqui.
 */
const API_BASE_URL = "http://localhost:5284/api";

/**
 * Faz uma chamada à API do CashWise, anexando o token JWT (se existir)
 * automaticamente no header Authorization.
 *
 * @param {string} path - caminho relativo, ex.: "/Auth/login"
 * @param {RequestInit} [options]
 * @returns {Promise<Response>}
 */
async function apiFetch(path, options = {}) {
  const token = typeof getToken === "function" ? getToken() : null;

  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {}),
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });
}

/**
 * Extrai uma mensagem de erro legível de uma resposta de erro da API
 * (que pode vir como { mensagem: "..." } ou como ProblemDetails de validação).
 */
async function extrairMensagemDeErro(response, mensagemPadrao) {
  try {
    const corpo = await response.json();

    if (corpo?.mensagem) {
      return corpo.mensagem;
    }

    if (corpo?.errors) {
      return Object.values(corpo.errors).flat().join(" ");
    }

    if (corpo?.title) {
      return corpo.title;
    }
  } catch {
    // corpo vazio ou não é JSON — usa a mensagem padrão
  }

  return mensagemPadrao;
}
