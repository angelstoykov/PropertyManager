(function () {
    const clientTypeSelect = document.getElementById('clientType');
    const individualFields = document.getElementById('individualFields');
    const companyFields = document.getElementById('companyFields');

    if (!clientTypeSelect || !individualFields || !companyFields) {
        return;
    }

    const individualValue = '10';

    function setSection(section, visible) {
        section.style.display = visible ? 'block' : 'none';
        section.querySelectorAll('input, select, textarea').forEach(function (element) {
            element.disabled = !visible;
        });
    }

    function toggleFields() {
        const isIndividual = clientTypeSelect.value === individualValue;
        setSection(individualFields, isIndividual);
        setSection(companyFields, !isIndividual);
    }

    clientTypeSelect.addEventListener('change', toggleFields);
    toggleFields();
})();
