(function () {
    const propertySelect = document.getElementById('propertySelect');
    const unitSelect = document.getElementById('unitSelect');

    if (!propertySelect || !unitSelect) {
        return;
    }

    const defaultUnitOption = '<option value="">-- Select unit --</option>';

    function resetUnitSelect() {
        unitSelect.innerHTML = defaultUnitOption;
        unitSelect.value = '';
        unitSelect.disabled = true;
    }

    propertySelect.addEventListener('change', async function () {
        const propertyId = propertySelect.value;

        if (!propertyId) {
            resetUnitSelect();
            return;
        }

        unitSelect.disabled = true;
        unitSelect.innerHTML = '<option value="">Loading...</option>';

        try {
            const response = await fetch(`/Clients/GetAvailableUnits?propertyId=${encodeURIComponent(propertyId)}`);

            if (!response.ok) {
                throw new Error('Failed to load units.');
            }

            const units = await response.json();

            unitSelect.innerHTML = defaultUnitOption;

            if (units.length === 0) {
                unitSelect.innerHTML += '<option value="" disabled>No available units</option>';
                unitSelect.disabled = true;
                return;
            }

            for (const unit of units) {
                const option = document.createElement('option');
                option.value = unit.id;
                option.textContent = `${unit.unitNumber} (Floor ${unit.floor}, ${unit.area} m²)`;
                unitSelect.appendChild(option);
            }

            unitSelect.disabled = false;
        } catch {
            resetUnitSelect();
            unitSelect.innerHTML = '<option value="">Failed to load units</option>';
        }
    });
})();
