# HmacManager Client Library

This is the client library for ASP.NET Core [`HmacManager`](../../../src/README.md). Usage is very similar to that package.

    const hmacManagerFactory = new HmacManagerFactory([{
        name: "MyPolicy",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: "sha-256",
        signatureHashAlgorithm: "sha-256",
        schemes: [{
            name: "MyScheme",
            headers: ["X-AccountId", "X-Email"]
        }]
    }]);

    const hmacManager = hmacManagerFactory.create("MyPolicy", /*"MyScheme"*/);
    const signingResult = await hmacManager.sign(request);