function updateQuantity(cartId, newQuantity) {

    fetch(`/Cart/UpdateQuantity?id=${cartId}&quantity=${newQuantity}`, {
        method: 'Post',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(response => {
            if (response.success) {
                showAlertModal("Item Updated", "success");
                loadCart();
            } else {
                showAlertModal("Something went wrong", "error");
            }

        });
}