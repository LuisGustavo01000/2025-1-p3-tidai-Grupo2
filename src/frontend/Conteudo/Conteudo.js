/* Conteudo.js
 * – Insere novos cards na grade ao submeter o formulário
 * – Marca ownership por ID do usuário (não por nome)
 */

const CASHWISE_USUARIO_KEY = 'CASHWISE_USUARIO_KEY';

document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('contentForm');
  const grid = document.querySelector('.grid-conteudos');

  if (!form || !grid) return;

  form.addEventListener('submit', e => {
    e.preventDefault();

    const title = form.contentTitle.value.trim();
    const desc = form.contentDesc.value.trim();

    if (!title) return;

    const usuarioAtual = obterUsuarioAtual();
    const autorId = usuarioAtual?.id ?? null;

    const card = document.createElement('div');
    card.className = 'item dynamic';
    card.dataset.autorId = autorId ? String(autorId) : '';

    const h3 = document.createElement('h3');
    h3.textContent = title;

    const p = document.createElement('p');
    p.textContent = desc || 'Sem descrição.';

    card.appendChild(h3);
    card.appendChild(p);

    const isOwner = autorId !== null && autorId === usuarioAtual?.id;
    if (isOwner) {
      card.dataset.isOwner = 'true';
      const deleteButton = document.createElement('button');
      deleteButton.type = 'button';
      deleteButton.className = 'btn btn-sm btn-outline-danger mt-2';
      deleteButton.textContent = 'Excluir';
      deleteButton.addEventListener('click', () => card.remove());
      card.appendChild(deleteButton);
    }

    grid.appendChild(card);

    form.reset();
    form.contentTitle.focus();
  });
});

function obterUsuarioAtual() {
  try {
    const raw = localStorage.getItem(CASHWISE_USUARIO_KEY);
    const sessao = raw ? JSON.parse(raw) : null;
    return sessao?.usuario ?? null;
  } catch {
    return null;
  }
}
