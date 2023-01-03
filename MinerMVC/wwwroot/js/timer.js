const oneSecond = 1000;
const oneMinutes = oneSecond * 60;
const oneHour = oneMinutes * 60;


$(document).ready(() => {
    $("#start").click(() => {
        playStartSound();
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
            playTimeUpSound();
        }

        countDownTime -= oneSecond;
    }, oneSecond);
}

function playStartSound() {
    playMusic("/sounds/start-computeraif-14572.mp3");
}

function playTimeUpSound() {
    playMusic("/sounds/badge-coin-win-14675.mp3");
}

function playMusic(source) {
    const audio = document.createElement("audio");
    audio.src = source;
    audio.play();
}
