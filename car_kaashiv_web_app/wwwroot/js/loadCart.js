


function loadCart() {
    console.log("Script load in button click");
    const cartContent = document.getElementById("cartContent");
    const cartModal = new bootstrap.Modal(document.getElementById("cartModal"));

    console.log("Fetching cart...");

    fetch("/Cart/GetCartPartial")
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.text();
             })
             .then(html=>{
                 cartContent.innerHTML = html;
                 cartModal.show();
             }).catch(error => {
                 cartContent.innerHTML = "error";
                 console.log("Fetch error",error)
             });
        }
