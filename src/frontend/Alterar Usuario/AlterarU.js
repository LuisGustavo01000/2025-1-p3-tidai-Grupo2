/*
 * AterarU.js
 * – Carrega os dados reais do usuário autenticado
 * – Salva nome/e-mail via PUT /api/Usuario/me
 * – Altera senha via PUT /api/Usuario/me/senha
 * – Preview de foto continua só local (não existe upload no backend ainda)
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();
  montarNav('nav-links');

  const profileImage = document.getElementById('profileImage');
  const imageInput = document.getElementById('imageInput');
  const btnChangeImg = document.getElementById('changeImage');

  const dadosForm = document.getElementById('dados-form');
  const dadosFeedback = document.getElementById('dados-feedback');
  const inputNome = document.getElementById('newName');
  const inputEmail = document.getElementById('newEmail');

  const senhaForm = document.getElementById('senha-form');
  const senhaFeedback = document.getElementById('senha-feedback');
  const inputSenhaAtual = document.getElementById('currentPassword');
  const inputNovaSenha = document.getElementById('newPassword');
  const inputConfirmarSenha = document.getElementById('confirmPassword');

  function mostrarFeedback(box, mensagem, tipo = 'danger') {
    box.textContent = mensagem;
    box.className = `alert alert-${tipo}`;
    box.classList.remove('d-none');
  }

  /* === trocar foto (preview local) ========================== */
  btnChangeImg.addEventListener('click', () => imageInput.click());

  imageInput.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (ev) => { profileImage.src = ev.target.result; };
    reader.readAsDataURL(file);
  });

  /* === carregar dados atuais =================================== */
  async function carregarDadosAtuais() {
    try {
      const response = await apiFetch('/Usuario/me');
      if (!response.ok) return;

      const usuario = await response.json();
      inputNome.value = usuario.nome;
      inputEmail.value = usuario.email;
    } catch {
      mostrarFeedback(dadosFeedback, 'Não foi possível carregar seus dados atuais.');
    }
  }

  /* === salvar nome/e-mail ======================================= */
  dadosForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    dadosFeedback.classList.add('d-none');

    const botao = dadosForm.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/Usuario/me', {
        method: 'PUT',
        body: JSON.stringify({
          nome: inputNome.value.trim(),
          email: inputEmail.value.trim(),
        }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível salvar seus dados.');
        mostrarFeedback(dadosFeedback, mensagem);
        return;
      }

      mostrarFeedback(dadosFeedback, 'Dados atualizados com sucesso!', 'success');
    } catch {
      mostrarFeedback(dadosFeedback, 'Não foi possível conectar ao servidor do CashWise.');
    } finally {
      botao.disabled = false;
    }
  });

  /* === alterar senha ============================================= */
  senhaForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    senhaFeedback.classList.add('d-none');

    const novaSenha = inputNovaSenha.value.trim();
    const confirmar = inputConfirmarSenha.value.trim();

    if (novaSenha !== confirmar) {
      mostrarFeedback(senhaFeedback, 'As senhas não coincidem.');
      return;
    }

    const botao = senhaForm.querySelector('button[type="submit"]');
    botao.disabled = true;

    try {
      const response = await apiFetch('/Usuario/me/senha', {
        method: 'PUT',
        body: JSON.stringify({
          senhaAtual: inputSenhaAtual.value,
          novaSenha,
        }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível alterar sua senha.');
        mostrarFeedback(senhaFeedback, mensagem);
        return;
      }

      mostrarFeedback(senhaFeedback, 'Senha alterada com sucesso!', 'success');
      senhaForm.reset();
    } catch {
      mostrarFeedback(senhaFeedback, 'Não foi possível conectar ao servidor do CashWise.');
    } finally {
      botao.disabled = false;
    }
  });

  carregarDadosAtuais();
});
