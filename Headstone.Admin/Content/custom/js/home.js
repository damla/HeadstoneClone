//Chart İnitiailize
var ctx = document.getElementById('myChart').getContext('2d');
var myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['11/9', '11/20', '11/21', '11/22', '11/23', '11/24', '11/25'],
        datasets: [{
            label: 'Değer',
            
            data: [12, 19, 3, 5, 2, 3, 8],
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgba(255, 99, 132, 1)',
            borderWidth: 1,
            
        }]
    },
    options: {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        },
        title: {
            display: true,
            text: 'Satış Miktarı',
            fontSize: 25
        },
        legend: {
            display: true,
            position: 'top',
            labels: {
                fontColor: '#000'
            }
        },
        layout: {
            padding: {
                left: 10,
                right: 0,
                bottom: 0,
                top: 0
            }
        },
        tooltips: {
            enabled: true
        },
    }
});





//Selecting date filter processes
$(document).ready(function () {
    $('form').on('submit', function (e) {


        e.preventDefault();


        var startDate = $('#startDateTime').datepicker({ dateFormat: 'DD.MM.YYYY' }).val();
        var finishDate = $('#finishDateTime').datepicker({ dateFormat: 'DD.MM.YYYY' }).val();


        //For StartDate Split operation
        var startDateSplit = startDate.split(".");
        var startDay = parseInt(startDateSplit[0]);
        var startMonth = parseInt(startDateSplit[1]);
        var startYear = parseInt(startDateSplit[2]);

        //For FinishDate Split operation
        var finishDateSplit = finishDate.split(".");
        var finishDay = parseInt(finishDateSplit[0]);
        var finishMonth = parseInt(finishDateSplit[1]);
        var finishYear = parseInt(finishDateSplit[2]);

        var rangeStart = startDate;
        var rangeEnd = finishDate;



    });

});