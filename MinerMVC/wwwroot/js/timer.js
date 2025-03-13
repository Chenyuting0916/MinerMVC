const oneSecond = 1000;
const oneMinutes = oneSecond * 60;
const oneHour = oneMinutes * 60;

$(document).ready(() => {
    $("#startButton").click(() => {
        playStartSound();
        startCountDown();
        $("#startButton").prop('disabled', true);
    });
});

function startCountDown() {
    let countDownTime = 30 * 60 * 1000;
    let timerDisplay = document.getElementById("demo");

    let x = setInterval(function () {
        let minutes = Math.floor((countDownTime % oneHour) / oneMinutes);
        let seconds = Math.floor((countDownTime % oneMinutes) / oneSecond);

        // 格式化顯示，確保個位數前面有0
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;

        timerDisplay.innerHTML = minutes + ":" + seconds;

        if (countDownTime <= 0) {
            clearInterval(x);
            playTimeUpSound();
            timerDisplay.innerHTML = "00:00";
            $("#startButton").prop('disabled', false);
            return;
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