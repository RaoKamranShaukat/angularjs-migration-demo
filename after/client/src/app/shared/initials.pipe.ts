import { Pipe, PipeTransform } from '@angular/core';

// "Ayesha Khan" -> "AK". Used for the avatar circles.
// BEFORE: .filter('initials', ...) in app.module.js.  AFTER: a pipe.
@Pipe({ name: 'initials' })
export class InitialsPipe implements PipeTransform {
  transform(name: string | null | undefined): string {
    return (name ?? '')
      .trim()
      .split(/\s+/)
      .slice(0, 2)
      .map((part) => part.charAt(0).toUpperCase())
      .join('');
  }
}
