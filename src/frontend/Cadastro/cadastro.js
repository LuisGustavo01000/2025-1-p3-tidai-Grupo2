/*  cadastro.js
 *  – Valida o formulário de cadastro
 *  – Envia o cadastro para a API real do CashWise (POST /api/Auth/register)
 */

document.addEventListener('DOMContentLoaded', () => {

  const form = document.getElementById('signup-form');
  if (!form) return;

  form.addEventListener('submit', async e => {
    e.preventDefault();

    if (!validateSignup()) {
      return;
    }

    const nome = form.name.value.trim();
    const email = form.email.value.trim();
    const senha = form.password.value.trim();

    const botao = form.querySelector('button[type="submit"]');
    if (botao) {
      botao.disabled = true;
      botao.dataset.textoOriginal = botao.textContent;
      botao.textContent = 'Cadastrando...';
    }

    try {
      const response = await apiFetch('/Auth/register', {
        method: 'POST',
        body: JSON.stringify({ nome, email, senha }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(
          response,
          'Não foi possível concluir o cadastro. Tente novamente.'
        );
        alert(mensagem);
        return;
      }

      alert('Cadastro realizado com sucesso! Faça login para continuar.');
      window.location.href = '../Login/Login.HTML';
    } catch (erro) {
      alert('Não foi possível conectar ao servidor do CashWise. Verifique se a API está rodando.');
    } finally {
      if (botao) {
        botao.disabled = false;
        botao.textContent = botao.dataset.textoOriginal;
      }
    }
  });

  /* ===== Funções de validação ===== */
  function validateSignup() {
    const name            = form.name.value.trim();
    const email           = form.email.value.trim();
    const password        = form.password.value.trim();
    const confirmPassword = form['confirm-password'].value.trim();

    if (!name || !email || !password || !confirmPassword) {
      alert('Por favor, preencha todos os campos.');
      return false;
    }

    if (password.length < 8) {
      alert('A senha deve ter no mínimo 8 caracteres.');
      return false;
    }

    if (!/\d/.test(password)) {
      alert('A senha deve conter pelo menos um número.');
      return false;
    }

    if (password !== confirmPassword) {
      alert('As senhas não coincidem.');
      return false;
    }

    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
      alert('Por favor, insira um e-mail válido.');
      return false;
    }

    return true;
  }
});
