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
            accessKey: 'dc9cd4ac9a11639ba404273d7a702ffc0694b24ab42108c1cd4be7f4676535d8',
            secretKey: 'cae7b345386cf95863de78b9ce9552bc5bbf14aecac5412d041aaf8bbb6bf4b81223809dc64093093a3e3102cd97f833',
            contractId: '424367c0-ec1a-11ea-b2a3-83b1a2aaa1f2',
            userId: 'hanming-production',
        }
    },
    signans: {
        host: 'wss://translate.signans.io',
        authConfig: {
            accessKey: '97a752d825fa2f66a6375747e018f7ca79402faf5a93fdb0ec08f2142ab78b5d',
            secretKey: 'd4669fa959d433bcfbc5900f76398833de2c69897eee6d1b74794df3f08dedbe69e965ae67c203b50729898081fff78e',
            contractId: 'c88f7cf0-6691-11eb-acc3-815c59b83180',
            userId: 'hanming-production',
        }
    },
    signansStg: {
        host: 'ws://staging-translate.signans.info',
        authConfig: {
            accessKey: '8dafdb58118e3773d2abb1b2f11969b9c29754764496720a0a83187ed34e255f',
            secretKey: '502425b04617e76fb882c89709665ce02b220c0ca529aea08d129eb9305dd478423dbcba9b4cc87f02b90088f8dedfa0',
            contractId: 'a670d7b0-66bc-11eb-aba9-658b8f7fde55',
            userId: 'hanming-staging',
        }
    }
};

module.exports = envConfigs;