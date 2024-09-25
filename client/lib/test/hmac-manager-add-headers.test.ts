import { assert, test } from "vitest";
import { HmacManager } from "../hmac_manager/hmac-manager.js";

test("HmacManager_Sign_Adds_Authorization_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: "sha-256",
        signatureHashAlgorithm: "sha-256",
        schemes: []
    });

    const signingResult = await hmacManager.sign(request);
    const authorizationHeader = request.headers.get("Authorization");
    const authorizationHeaderSignature = authorizationHeader?.split("Hmac ")[1];
    assert.equal(signingResult.hmac?.signature, authorizationHeaderSignature);
});

test("HmacManager_Sign_Adds_DateRequested_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: "sha-256",
        signatureHashAlgorithm: "sha-256",
        schemes: []
    });

    const signingResult = await hmacManager.sign(request)
    const dateRequestedHeader = request.headers.get("Hmac-Date-Requested");
    assert.equal(signingResult.hmac?.dateRequested.getTime().toString(), dateRequestedHeader);
});

test("HmacManager_Sign_Adds_Nonce_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: "sha-256",
        signatureHashAlgorithm: "sha-256",
        schemes: []
    });

    const signingResult = await hmacManager.sign(request)
    const nonceHeader = request.headers.get("Hmac-Nonce");
    assert.equal(signingResult.hmac?.nonce, nonceHeader);
});