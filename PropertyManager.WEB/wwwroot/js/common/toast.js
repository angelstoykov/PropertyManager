window.showToast = function (message, type = 'success') {

    const toastEl = document.getElementById('appToast');
    const toastBody = document.getElementById('appToastBody');

    toastBody.innerText = message;

    toastEl.classList.remove('bg-success', 'bg-danger', 'bg-warning');

    switch (type) {
        case 'error':
            toastEl.classList.add('bg-danger');
            break;
        case 'warning':
            toastEl.classList.add('bg-warning');
            toastEl.classList.add('text-dark');
            break;
        default:
            toastEl.classList.add('bg-success');
    }

    const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
    toast.show();
}
