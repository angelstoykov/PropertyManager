let unitIdToDelete = null;
let confirmModal = null;

document.addEventListener('DOMContentLoaded', () => {

    confirmModal = new bootstrap.Modal(
        document.getElementById('confirmDeleteModal')
    );

    document.querySelectorAll('.js-delete-unit')
        .forEach(btn => {
            btn.addEventListener('click', () => {

                unitIdToDelete = btn.dataset.unitId;
                const unitName = btn.dataset.unitName;

                document.getElementById('confirmDeleteMessage')
                    .innerText = `Are you sure you want to delete unit "${unitName}"?`;

                confirmModal.show();
            });
        });

    document.getElementById('confirmDeleteBtn')
        .addEventListener('click', deleteConfirmed);
});

function deleteConfirmed() {

    const deleteBtn = document.getElementById('confirmDeleteBtn');
    deleteBtn.disabled = true;
    deleteBtn.innerText = 'Deleting...';

    fetch(`/api-proxy/units/${unitIdToDelete}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error();
            }

            showToast('Unit deleted successfully.');
            setTimeout(() => location.reload(), 800);
        })
        .catch(err => {
            showToast('Error deleting unit.', 'error');
        })
        .finally(() => {
            deleteBtn.disabled = false;
            deleteBtn.innerText = 'Delete';
            confirmModal.hide();
        });
}
