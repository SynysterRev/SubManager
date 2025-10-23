import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'localDate'
})
export class LocalDatePipe implements PipeTransform {

  transform(value: string | Date, options?: Intl.DateTimeFormatOptions): string {
    if (!value) return '';
    const date = new Date(value);
    return new Intl.DateTimeFormat(navigator.language, options).format(date);
  }

}
