const WebSocket = require('ws');
const fs = require('fs');

const runClient = (roomUrl, audioFile) => {
    const connection = new WebSocket(roomUrl);
    connection.onopen = (ev) => {
        console.log('Connected with Onyaku chatroom');
    };

    connection.onmessage = (ev) => {
        const jsonMessage = JSON.parse(ev.data);
        //console.log(jsonMessage);
        const { type, code, message } = jsonMessage;
        if (type === 'operation') {
            switch (code) {
                case 1:
                    console.log('Joined:', message);
                    break;
                case 2:
                    const translateSelf = {
                        type: 'command',
                        code: 4,
                        message: 'en'
                    }
                    connection.send(JSON.stringify(translateSelf), (err) => {
                        if (err) {
                            console.error(err);
                        } else {
                            console.log('Set translate to self');
                        }
                    });
                    fs.createReadStream(audioFile, { highWaterMark: 8192 })
                        .on('data', (buf) => {
                            //console.log(`Buffer length: ${buf.length}`);
                            connection.send(buf, (error) => {
                                if (error) {
                                    console.error(error.message);
                                }
                            });
                        }).on('end', () => {
                            console.log('Audio data read finished');
                        });
                    break;
                case 3:
                    console.log('Left:', message);
                    break;
            }
        } else if (type === 'speech' || type === 'translation') {
            const messageJson = JSON.parse(message);
            console.log(type, ':', messageJson);
        } else {
            console.log(message);
        }
    }

    connection.onclose = (ev) => {
        console.log('Connection closed by Onyaku:', ev.code, ev.reason);
    }
}

const inputs = [
    {
        roomUrl: "wss://staging-onyaku-chatroom.signans.info/room?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI5NmZiZTM2ZWViNzc5NzZiIiwiZW1haWwiOiJyZXMucm96ZXR0YUBnbWFpbC5jb20iLCJ1c2VyTmFtZSI6InJlcy5yb3pldHRhQGdtYWlsLmNvbSIsImlhdCI6MTYzOTk4OTE2NX0.yEjPUpV3twjHRLGO0DkedQdr3eGJktF_1vTuzD868_M&roomId=y77CHW6&language=ja",
        audioFile: 'ja_jp.wav'
    },
    /*{
        token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJiODRiZWU2Ni1lN2M3LTQwOGMtYmVmOC1mZDMyZjEyMzc3ZTkiLCJlbWFpbCI6Imgtd3VAcm96ZXR0YS5qcCIsInVzZXJOYW1lIjoiaC13dUByb3pldHRhLmpwIiwiaWF0IjoxNjM5NDU5Mzg3fQ.yXcUElKlpsLjKNGVg-btJujB-6Ovr8AlVHV2wjait8Q",
        language: "en",
        roomId: "FWfsHsT",
        audioFile: 'en_us.wav'
    },*/
]
inputs.forEach(input => {
    const { roomUrl, audioFile } = input;
    runClient(roomUrl, audioFile);
})