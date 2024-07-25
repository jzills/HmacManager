import { equal } from "assert";
import { expect, test } from "vitest";
import { HmacProvider } from "../hmac_manager/components/hmac-provider.js";

test("HmacProvider", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast", {
        method: "get",
        headers: {
            "X-Account": "123",
            "X-Email": "my@email.com"
        }
    });

    const publicKey = "eb8e9dae-08bd-4883-80fe-1d9a103b30b5";
    const privateKey = btoa("thisIsMySuperCoolPrivateKey");
    const signedHeaders = ["X-Account", "X-Email"];
    const hmacProvider = new HmacProvider(publicKey, privateKey, signedHeaders);

    const dateRequested = new Date("7/24/2024 7:31:19.232 PM +00:00");
    const nonce = "e21a00ff-585c-4c29-976a-fcea069be118";
    const { signature, signingContent } = await hmacProvider.compute(request, dateRequested, nonce);

    const expectedSignature = "lg8T5yPtU6+T9zaRM4EvzDmj/5RPK4UP5RP9xsQqbZo=";
    const expectedSigningContent = "GET:/api/weatherforecast:localhost:7216:1721849479232:eb8e9dae-08bd-4883-80fe-1d9a103b30b5:123:my@email.com:e21a00ff-585c-4c29-976a-fcea069be118";

    expect(equal(signature, expectedSignature));
    expect(equal(signingContent, expectedSigningContent));
})