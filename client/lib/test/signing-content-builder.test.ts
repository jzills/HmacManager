import { assert, test } from "vitest";
import SigningContentBuilder from "../src/builders/signing-content-builder";

test("SigningContentBuilder", async () => {
    const dateRequested = new Date();
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const builder = new SigningContentBuilder().createBuilder()
        .withRequest(request)
        .withPublicKey("SOME_PUBLIC_KEY")
        .withDateRequested(dateRequested)
        .withContentHash("SOME_CONTENT_HASH")
        .withSignedHeaders([])
        .withNonce("SOME_NONCE");

    const signingContent = await builder.build();
    assert.equal(signingContent, `GET:/api/weatherforecast:localhost:7216:${dateRequested.getTime()}:SOME_PUBLIC_KEY:SOME_CONTENT_HASH:SOME_NONCE`)
});