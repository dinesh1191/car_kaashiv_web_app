const { Modal } = require("bootstrap");

window.ProceedCheckout = async function () {
    try {
        const response = await fetch(`/Cart/Checkout`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) throw new Error('Server error: ${ response.status }');
        const result = await response.json();
        if (result.success) {            
            showToast(result.message, result.orderId, "success");
            console.log("Checkout successful. Order ID:", result.orderId);
        }
        else {

            showToast(result.orderId, "error");
            console.log("Checkout failed. Message:", result.orderId);
        }
    }
    catch (err) {
        showToast("Network error occured", "error");
    }
};


