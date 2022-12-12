const oneSecond = 1000;
const oneMinutes = oneSecond * 60;
const oneHour = oneMinutes * 60;


$(document).ready(() => {
    $("#start").click(() => {
        startCountDown();
        $("#start").remove();
    });
});


function startCountDown() {
    let countDownTime = 30 * 60 * 1000;

    let x = setInterval(function () {

        let minutes = Math.floor((countDownTime % oneHour) / oneMinutes);
        let seconds = Math.floor((countDownTime % oneMinutes) / oneSecond);

        document.getElementById("demo").innerHTML = minutes + "m " + seconds + "s ";

        if (countDownTime <= 0) {
            clearInterval(x);
        }

        countDownTime -= oneSecond;
    }, oneSecond);
}

