// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(() => {
    let connection = new signalR.HubConnectionBuilder().withUrl("Hubs/CusHub").build()

    connection.start()

    connection.on("displayCustomer", function () {
        loadData()
    })

    loadData();

    function loadData() {
        var tr = ''

        $.ajax({
            url: '/CustomerInfoes',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr = tr + `<tr>
                        <td>${v.Id}</td>
                        <td>${v.CusId}</td>
                        <td>${v.CusName}</td>
                    </tr>`
                })

                $("#tableBody").html(tr)
            },
            error: (error) => {
                console.log(error)
            }
        })
    }
})