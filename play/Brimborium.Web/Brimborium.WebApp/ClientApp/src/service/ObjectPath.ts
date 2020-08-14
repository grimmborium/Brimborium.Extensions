import {ObjectPathMode,ObjectPropertyValue} from './types';

export class ObjectPath {
    public basePath? : ObjectPath;
    public part? : ObjectPropertyValue;
    constructor(
        basePath? : ObjectPath,
        part? : ObjectPropertyValue) {
        this.basePath = basePath;
        this.part = part;
    }

    public next(value:ObjectPropertyValue):ObjectPath{
        return new ObjectPath(this, value);
    }

    public nexts(values:ObjectPropertyValue[]):ObjectPath{
        var result:ObjectPath=this;
        for (let index = 0; index < values.length; index++) {
            const part = values[index];
            result=new ObjectPath(result, part);
        }
        return result;
    }

    public completeParts(result?:ObjectPropertyValue[]):ObjectPropertyValue[] {
        const parts:ObjectPropertyValue[]=(result)?result:[];
        if (this.basePath) {
            this.basePath.completeParts(parts);
        }
        if (this.part) {
            parts.push(this.part);
        }
        return parts;
    }
    public static __Root? :ObjectPath;
    public static getRoot() :ObjectPath {
        return (ObjectPath.__Root) 
            ? ObjectPath.__Root 
            : (ObjectPath.__Root=new ObjectPath());
    }
}