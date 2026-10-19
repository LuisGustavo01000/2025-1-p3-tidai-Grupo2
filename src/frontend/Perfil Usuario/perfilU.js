const CASHWISE_USUARIO_KEY = 'CASHWISE_USUARIO_KEY';

document.addEventListener('DOMContentLoaded', async () => {
  const $ = id => document.getElementById(id);

  const modal = $('modal');
  const modalContent = modal?.querySelector('.modal-content');
  const btnEdit = $('editProfile');
  const btnClose = modal?.querySelector('.close');
  const btnSave = $('saveProfile');

  const spanName = $('user-name');
  const spanAge = $('user-age');
  const spanPeriod = $('user-period');

  const inName = $('editName');
  const inAge = $('editAge');
  const inPeriod = $('editPeriod');

  let usuarioCarregado = null;
  let carregando = true;

  const atualizarEstadoEdicao = () => {
    const desabilitar = carregando || !usuarioCarregado?.email;
    if (btnEdit) btnEdit.disabled = desabilitar;
    if (btnSave) btnSave.disabled = desabilitar;
  };

  const showModal = () => {
    if (carregando || !usuarioCarregado?.email || !modal || !inName || !inAge || !inPeriod || !spanName || !spanAge || !spanPeriod) {
      return;
    }

    inName.value = spanName.textContent.trim();
    inAge.value = spanAge.textContent.replace(/\D/g, '');
    inPeriod.value = spanPeriod.textContent.trim();

    modal.classList.add('d-flex');
    inName.focus();
  };

  const hideModal = () => {
    modal?.classList.remove('d-flex');
  };

  const saveProfile = () => {
    if (!usuarioCarregado?.email || !inName || !inAge || !inPeriod || !spanName || !spanAge || !spanPeriod) {
      alert('Usuário ainda não carregado.');
      return;
    }

    const name = inName.value.trim();
    const age = parseInt(inAge.value.trim(), 10);
    const period = inPeriod.value.trim();

    if (name) spanName.textContent = name;
    if (!isNaN(age) && age > 0) spanAge.textContent = `${age} anos`;
    if (period) spanPeriod.textContent = period;

    const sessaoAtual = obterSessaoAtual();
    const usuarioAtualizado = {
      ...(sessaoAtual?.usuario ?? {}),
      nome: spanName.textContent.trim(),
      idade: age,
      periodo: spanPeriod.textContent.trim(),
      email: usuarioCarregado.email
    };

    salvarSessaoComUsuario(usuarioAtualizado, sessaoAtual);
    hideModal();
  };

  btnEdit?.addEventListener('click', showModal);
  btnClose?.addEventListener('click', hideModal);
  btnSave?.addEventListener('click', saveProfile);

  modal?.addEventListener('click', e => {
    if (modalContent && !modalContent.contains(e.target)) hideModal();
  });

  document.addEventListener('keydown', e => {
    if (e.key === 'Escape' && modal?.classList.contains('d-flex')) hideModal();
  });

  atualizarEstadoEdicao();

  usuarioCarregado = await carregarUsuario(spanName, spanAge, spanPeriod);
  carregando = false;
  atualizarEstadoEdicao();
});

async function carregarUsuario(spanName, spanAge, spanPeriod) {
  const sessao = obterSessaoAtual();
  const usuario = sessao?.usuario ?? null;

  if (usuario?.nome && spanName) spanName.textContent = usuario.nome;
  if (usuario?.idade && spanAge) spanAge.textContent = `${usuario.idade} anos`;
  if (usuario?.periodo && spanPeriod) spanPeriod.textContent = usuario.periodo;

  return usuario;
}

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
