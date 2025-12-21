function deleteUnit(id) {
    if (!confirm('Are you sure you want to delete this unit?')) {
        return;
    }

    fetch(`/api-proxy/units/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert('Error deleting unit.');
            }
        });
}
