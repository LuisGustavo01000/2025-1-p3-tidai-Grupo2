/*
 * PerfilU.js — antes mostrava nome/idade/período fixos no HTML e "salvava"
 * só localmente (sumia ao recarregar). Agora carrega o usuário e o resumo
 * financeiro reais da API; idade/período foram removidos porque não existem
 * no modelo real de usuário (inventar esses campos exigiria uma migration
 * que ninguém pediu — mais simples e honesto mostrar só o que é real).
 */
document.addEventListener('DOMContentLoaded', () => {
  exigirLogin();
  montarNav('nav-links', 'perfil');

  const $ = (id) => document.getElementById(id);

  const modal = $('modal');
  const modalContent = modal.querySelector('.modal-content');
  const modalErro = $('modal-erro');
  const btnEdit = $('editProfile');
  const btnClose = modal.querySelector('.close');
  const btnSave = $('saveProfile');

  const spanName = $('user-name');
  const spanEmail = $('user-email');
  const inName = $('editName');

  const statSaldo = $('stat-saldo');
  const statGastos = $('stat-gastos');
  const statMetas = $('stat-metas');
  const perfilErro = $('perfil-erro');

  let emailAtual = '';

  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  function mostrarErroPerfil(mensagem) {
    perfilErro.textContent = mensagem;
    perfilErro.classList.remove('d-none');
  }

  function mostrarErroModal(mensagem) {
    modalErro.textContent = mensagem;
    modalErro.classList.remove('d-none');
  }

  async function carregarUsuario() {
    try {
      const response = await apiFetch('/Usuario/me');
      if (!response.ok) {
        mostrarErroPerfil('Não foi possível carregar seus dados agora.');
        return;
      }

      const usuario = await response.json();
      spanName.textContent = usuario.nome;
      spanEmail.textContent = usuario.email;
      emailAtual = usuario.email;
    } catch {
      mostrarErroPerfil('Não foi possível conectar ao servidor do CashWise.');
    }
  }

  async function carregarResumo() {
    try {
      const response = await apiFetch('/Dashboard/resumo');
      if (!response.ok) return;

      const resumo = await response.json();
      statSaldo.textContent = formatarMoeda(resumo.saldoTotal);
      statSaldo.classList.toggle('text-success', resumo.saldoTotal >= 0);
      statSaldo.classList.toggle('text-danger', resumo.saldoTotal < 0);
      statGastos.textContent = formatarMoeda(resumo.gastosMes);
      statMetas.textContent = resumo.metasAtivas;
    } catch {
      // cards ficam com "—" se a API falhar
    }
  }

  /* === modal de editar nome ================================== */
  const showModal = () => {
    modalErro.classList.add('d-none');
    inName.value = spanName.textContent.trim();
    modal.classList.add('d-flex');
    inName.focus();
  };

  const hideModal = () => modal.classList.remove('d-flex');

  const saveProfile = async () => {
    const novoNome = inName.value.trim();
    modalErro.classList.add('d-none');

    if (novoNome.length < 2) {
      mostrarErroModal('O nome precisa ter pelo menos 2 caracteres.');
      return;
    }

    btnSave.disabled = true;

    try {
      const response = await apiFetch('/Usuario/me', {
        method: 'PUT',
        body: JSON.stringify({ nome: novoNome, email: emailAtual }),
      });

      if (!response.ok) {
        const mensagem = await extrairMensagemDeErro(response, 'Não foi possível salvar o nome.');
        mostrarErroModal(mensagem);
        return;
      }

      spanName.textContent = novoNome;
      hideModal();
    } catch {
      mostrarErroModal('Não foi possível conectar ao servidor do CashWise.');
    } finally {
      btnSave.disabled = false;
    }
  };

  btnEdit.addEventListener('click', showModal);
  btnClose.addEventListener('click', hideModal);
  btnSave.addEventListener('click', saveProfile);

  modal.addEventListener('click', (e) => {
    if (!modalContent.contains(e.target)) hideModal();
  });

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && modal.classList.contains('d-flex')) hideModal();
  });

  carregarUsuario();
  carregarResumo();
});
