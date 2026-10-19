function exibirErro(mensagem) {
  alert(mensagem);
}

function validarCamposLogin(email, senha) {
  if (!email || !senha) {
    exibirErro("Por favor, preencha todos os campos.");
    return false;
  }

  const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
  if (!emailRegex.test(email)) {
    exibirErro("Por favor, insira um email válido.");
    return false;
  }

  return true;
}

async function realizarLogin(event) {
  event.preventDefault();

  const email = document.getElementById("email")?.value.trim();
  const senha = document.getElementById("password")?.value;

  if (!validarCamposLogin(email, senha)) {
    return;
  }

  const botao = document.querySelector('#login-form button[type="submit"]');
  if (botao) {
    botao.disabled = true;
    botao.dataset.textoOriginal = botao.textContent;
    botao.textContent = "Entrando...";
  }

  try {
    const response = await apiFetch("/Auth/login", {
      method: "POST",
      body: JSON.stringify({ email, senha }),
    });

    if (!response.ok) {
      const mensagem = await extrairMensagemDeErro(
        response,
        "Não foi possível entrar. Tente novamente."
      );
      exibirErro(mensagem);
      return;
    }

    const dados = await response.json();

    salvarSessao({
      token: dados.token,
      usuarioId: dados.usuarioId,
      nome: dados.nome,
      email: dados.email,
      expiraEmUtc: dados.expiraEmUtc,
    });

    window.location.href = "../Página do Usuario/Usuario.html";
  } catch (erro) {
    exibirErro(
      "Não foi possível conectar ao servidor do CashWise. Verifique se a API está rodando."
    );
  } finally {
    if (botao) {
      botao.disabled = false;
      botao.textContent = botao.dataset.textoOriginal;
    }
  }
}

document.getElementById("login-form")?.addEventListener("submit", realizarLogin);
