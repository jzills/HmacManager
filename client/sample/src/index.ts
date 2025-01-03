import { HmacManagerFactory, HashAlgorithm } from "hmac-manager";

const runSample = async () => {
    const hmacManagerFactory = new HmacManagerFactory([{
        name: "Some Policy",
        privateKey: "Some Private Key",
        publicKey: "Some Public Key",
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        contentHashAlgorithm: HashAlgorithm.SHA256,
        schemes: []
    }]);
    
    const hmacManager = hmacManagerFactory.create("Some Policy");
    if (hmacManager === null) {
        console.error("An error occurred creating an instance of \"HmacManager\".");
    } else {
        const result = await hmacManager.sign(new Request("https://google.com"));
        console.log(result);
    }
}

runSample();