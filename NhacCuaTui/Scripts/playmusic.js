const songList = document.getElementById('songList');
const audioPlayer = document.getElementById('audioPlayer');
const audioSource = document.getElementById('audioSource');

// Chọn bài hát từ danh sách
songList.addEventListener('click', function (e) {
    if (e.target && e.target.nodeName === "LI") {
        const songSrc = e.target.getAttribute('data-src');
        audioSource.src = songSrc;
        audioPlayer.load();
        audioPlayer.play();
    }
});
