export enum ObjectPathMode{
    Named, Indexed
}
export type ObjectPropertyValue = {
    mode :  ObjectPathMode.Named;
    value: string;
} | {
    mode :  ObjectPathMode.Indexed;
    value: number;
}