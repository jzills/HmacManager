import SigningContentAccessor from "./signing-content-accessor";
import SigningContentBuilder from "./signing-content-builder";

export default class SigningContentBuilderAccessor extends SigningContentBuilder {
    private readonly signingContentAccessor: SigningContentAccessor;

    constructor(signingContentAccessor: SigningContentAccessor) {
        super();
        this.signingContentAccessor = signingContentAccessor;
    }

    createBuilder = () => new SigningContentBuilderAccessor(this.signingContentAccessor);

    build = () => this.signingContentAccessor(this.context);
}