(function () {
    const clientTypeSelect = document.getElementById('clientType');
    const individualFields = document.getElementById('individualFields');
    const legalEntityFields = document.getElementById('legalEntityFields');

    if (!clientTypeSelect || !individualFields || !legalEntityFields) {
        return;
    }

    const individualValue = '10';
    const legalEntityValue = '20';

    function toggleFields() {
        const isIndividual = clientTypeSelect.value === individualValue;
        individualFields.style.display = isIndividual ? 'block' : 'none';
        legalEntityFields.style.display = isIndividual ? 'none' : 'block';
    }

    clientTypeSelect.addEventListener('change', toggleFields);
    toggleFields();
})();
