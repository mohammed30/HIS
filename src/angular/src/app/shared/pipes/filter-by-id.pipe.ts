import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterById',
  standalone: true
})
export class FilterByIdPipe implements PipeTransform {
  transform(items: any[], id: any, property: string = 'name'): any {
    if (!items || !id) {
      return '';
    }
    const item = items.find(i => i.id === id);
    return item ? item[property] : '';
  }
}
