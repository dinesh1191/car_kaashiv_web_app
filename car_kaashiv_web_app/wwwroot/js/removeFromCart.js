
window.removeFromCart = function (cartId) {
    console.log("Removing item:", cartId);
    fetch(`/Cart/RemoveFromCart?cartId=${cartId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(res => res.json())
        .then(response => {
            if (response.success) {
                showAlertModal("Item removed!", "success");
                loadCart();
            } else {
                showAlertModal(response.message, "error");
            }
        })
        .catch(err => {
            console.error("Error removing item:", err);
        });
};
