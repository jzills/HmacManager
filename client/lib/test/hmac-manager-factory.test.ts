import { assert, test } from "vitest";
import HmacManagerFactory from "../src/hmac-manager-factory";
import HashAlgorithm from "../src/hash-algorithm";

test("HmacManagerFactory", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast", {
        headers: {
            "X-AccountId": "123",
            "X-Email": "my@email.com"
        }
    });

    const hmacManagerFactory = new HmacManagerFactory([{
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        schemes: [{
            name: "Scheme",
            headers: ["X-AccountId", "X-Email"]
        }]
    }]);

    const hmacManager = hmacManagerFactory.create("Policy-A");
    await hmacManager?.sign(request);

    const policyHeader = request.headers.get("Hmac-Policy");
    const schemeHeader = request.headers.get("Hmac-Scheme");
    assert.equal(policyHeader, "Policy-A");
    assert.equal(schemeHeader, null);
});

test("HmacManagerFactory_With_SigningContentAccessor", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast", {
        headers: {
            "X-AccountId": "123",
            "X-Email": "my@email.com"
        }
    });

    const hmacManagerFactory = new HmacManagerFactory([{
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        schemes: [{
            name: "Scheme",
            headers: ["X-AccountId", "X-Email"]
        }],
        signingContentAccessor: context => Promise.resolve(`${context.request?.method}`) // Really bad idea, don't do it
    }]);

    const hmacManager = hmacManagerFactory.create("Policy-A");
    const signingResult = await hmacManager?.sign(request);

    const policyHeader = request.headers.get("Hmac-Policy");
    const schemeHeader = request.headers.get("Hmac-Scheme");
    assert.equal(policyHeader, "Policy-A");
    assert.equal(schemeHeader, null);
    assert.equal(signingResult?.hmac?.signingContent, "GET")
});