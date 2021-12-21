const envConfigs = {
    local: {
        host: 'ws://localhost:3000',
        authConfig: {
            accessKey: '8ba1f7a180ab38c85f11739f220e73a64da3ecc788c248f1594db02b124a046e',
            secretKey: '360baf7aac797eb7dafd97588cc7db5ab12487ef708e50bd66ad852a85425e2f4a3f868550e1e7427e8dc111bf81b254',
            contractId: 'd4022e50-595f-11eb-9be2-dd6b8bafae6a',
            userId: 'hanming-local',
        }
    },
    stg2: {
        host: 'wss://staging1.classiii.info',
        authConfig: {
            accessKey: 'a91ca9a65a076af060caf1a5dd2efb57b300ae4c68454a09f9fc72222f95d71e',
            secretKey: '8e211d4d64920c8169c6f413f73baa706b1625b668d31d5f94a81c0ddeea18b391432753f7a04dc82b5d05d753998608',
            contractId: '12b7fa50-d0b0-11ea-ad62-b568e8b6e6f6',
            userId: 'hanming-staging',
        }
    },
    info: {
        host: 'wss://translate.classiii.info',
        authConfig: {
            accessKey: 'dc9cd4ac9a11639ba404273d7a702ffc0694b24ab42108c1cd4be7f4676535d8',
            secretKey: 'cae7b345386cf95863de78b9ce9552bc5bbf14aecac5412d041aaf8bbb6bf4b81223809dc64093093a3e3102cd97f833',
            contractId: '424367c0-ec1a-11ea-b2a3-83b1a2aaa1f2',
            userId: 'hanming-production',
        }
    },
    io: {
        host: 'wss://translate.classiii.io',
        authConfig: {
            accessKey: '02cab37284a0b99119bfb2977e45d04524ec27205806b7f94cfd88cd61f75b28',
            secretKey: '2bac08584a845601d2d30bd6f4c441650674ae119030cdf11fbd8c9e0379872478febc4a2119e78e8148510f4dbecafc',
            contractId: '65da9df0-45c6-11ec-9520-afd1a8c435ce',
            userId: 'ACT_Developer',
        }
    },
    signans: {
        host: 'wss://translate.signans.io',
        authConfig: {
            accessKey: '3e7ebb778cb0467d91dd82fa083e3f62d455e017be30639dfa1106b546d4c4af',
            secretKey: '817e0cfdac8a9fd7c0c8330c2904c593929ea701848d7e37ee5f8ebd80cd3f3b0eed4d5d2db118ff4fceed5665d66a64',
            userId: 'rozetta-internal',
        }
    },
    signansStg: {
        host: 'wss://staging-translate.signans.info',
        authConfig: {
            accessKey: '6ceef9c6637d151fd62d80d00c9f8e28954a84f6ead72dd2db95751e2a084bf7',
            secretKey: 'f0d0b49eb07a5a02ddd986640c0dd161b5386d6b7843919f12080830d64e850aa8803ab810772a7aeec1fe3507873811',
            userId: 'GPC',
        }
    },
    signansTdx: {
        host: 'ws://staging-translate-tdx.signans.info',
        authConfig: {
            accessKey: '43e0912ee5507237b91c25385fcd923eb22d4dc999d16d1b00f01e6dee5a05bf',
            secretKey: '162be4df16f2153a8a57adcc94d69c49a5b21e4e7a660fb41930c280fee4713d7d7f23508e4c9479cc4f9cc732aec801',
            userId: 'traveldx-dev',
        }
    },
    onyakuLocal: {
        host: 'ws://localhost:3006',
    },
    onyakuStg: {
        host: 'ws://stg-chatroom-onyaku.ap-northeast-1.elasticbeanstalk.com'
    }
};

module.exports = envConfigs;