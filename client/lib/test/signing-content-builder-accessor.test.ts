import { assert, test } from "vitest";
import SigningContentAccessor from "../src/builders/signing-content-accessor";
import SigningContentBuilderAccessor from "../src/builders/signing-content-builder-accessor";

test("SigningContentBuilderAccessor", async () => {
    const contextAccessor: SigningContentAccessor = context => {
        const content = `GET:${context.dateRequested.getTime().toString()}:${context.publicKey}`;
        return Promise.resolve(content);
    }
    const dateRequested = new Date();
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const builder = new SigningContentBuilderAccessor(contextAccessor).createBuilder()
        .withRequest(request)
        .withPublicKey("SOME_PUBLIC_KEY")
        .withDateRequested(dateRequested)
        .withContentHash("SOME_CONTENT_HASH")
        .withSignedHeaders([])
        .withNonce("SOME_NONCE");

    const signingContent = await builder.build();
    assert.equal(signingContent, `GET:${dateRequested.getTime()}:SOME_PUBLIC_KEY`)
});