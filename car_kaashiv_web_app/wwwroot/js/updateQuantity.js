


window.updateQuantity = async function (cartId, newQuantity) {
    try {
        const response = await fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ cartId, quantity: newQuantity })
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            showToast("Item quantity updated successfully!", "success");
            await loadCart(); // refresh cart UI
        } else {
            showToast(result.message || "Failed to update quantity", "error");
        }

    } catch (err) {
        console.error("Error updating quantity:", err);
        showToast("Network error occurred. Please try again.", "error");
    }
};


