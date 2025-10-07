
// js with async await method
window.removeFromCart = async function (cartId) {  
    try {   
        const response = await fetch(`/Cart/RemoveFromCart`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cartId) // passing cartId on body to controller action
        });
        if (!response.ok) {
            throw Error(`Failed to remove item.Status: ${response.status}`);
        }
        const result = await response.json(); 
        showToast(result.message, "success");
        loadCart();  
    }
        catch(err) {
        console.log("Error removing item:", err);
        showToast("Something went wrong.Please try again.", "error");
        };

};
