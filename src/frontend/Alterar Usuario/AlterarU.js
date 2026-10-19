/* AlterarU.js
 * – Troca a foto do usuário (preview local)
 * – Valida e salva alterações
 */

const CASHWISE_USUARIO_KEY = 'CASHWISE_USUARIO_KEY';

document.addEventListener('DOMContentLoaded', () => {
  const profileImage = document.getElementById('profileImage');
  const imageInput = document.getElementById('imageInput');
  const btnChangeImg = document.getElementById('changeImage');
  const btnSave = document.getElementById('saveChanges');

  btnChangeImg?.addEventListener('click', () => imageInput?.click());

  imageInput?.addEventListener('change', e => {
    const file = e.target.files?.[0];
    if (!file || !profileImage) return;

    const reader = new FileReader();
    reader.onload = ev => {
      profileImage.src = ev.target?.result;
    };
    reader.readAsDataURL(file);
  });

  btnSave?.addEventListener('click', () => {
    const newName = document.getElementById('newName')?.value.trim() ?? '';
    const newPassword = document.getElementById('newPassword')?.value ?? '';
    const confirmPass = document.getElementById('confirmPassword')?.value ?? '';

    if (newPassword && newPassword.length < 8) {
      alert('A senha deve ter no mínimo 8 caracteres.');
      return;
    }

    if (newPassword && !/\d/.test(newPassword)) {
      alert('A senha deve conter pelo menos um número.');
      return;
    }

    if (newPassword && newPassword !== confirmPass) {
      alert('As senhas não coincidem!');
      return;
    }

    const sessaoAtual = obterSessaoAtual();
    const usuarioAtualizado = {
      ...(sessaoAtual?.usuario ?? {}),
      ...(newName ? { nome: newName } : {})
    };

    salvarSessaoComUsuario(usuarioAtualizado, sessaoAtual);

    alert(`Alterações salvas com sucesso!\nNome: ${newName || '(sem alteração)'}`);
  });
});

function obterSessaoAtual() {
  try {
    const raw = localStorage.getItem(CASHWISE_USUARIO_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function salvarSessaoComUsuario(usuario, sessaoAtual) {
  const novaSessao = {
    ...sessaoAtual,
    usuario,
    token: sessaoAtual?.token ?? null,
    expiracao: sessaoAtual?.expiracao ?? null
  };

  localStorage.setItem(CASHWISE_USUARIO_KEY, JSON.stringify(novaSessao));
}
