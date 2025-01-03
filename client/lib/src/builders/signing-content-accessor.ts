import SigningContentContext from "./signing-content-context";

type SigningContentAccessor = (context: SigningContentContext) => Promise<string>;

export default SigningContentAccessor;