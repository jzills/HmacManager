export interface IHeaderScheme {
    readonly name: string;
    addHeader(name: string);
    addHeader(name: string, claimType: string);
}