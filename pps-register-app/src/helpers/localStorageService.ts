
class StorageItem<T> {
  private key: string;

  constructor(key: string) {
    this.key = key;
  }

  get(): T | null {
    try {
      const item = localStorage.getItem(this.key);
      return item ? JSON.parse(item) as T : null;
    } catch (error) {
      console.error(`Error getting item from localStorage: ${error}`);
      return null;
    }
  }

  set(value: T): void {
    try {
      const serializedValue = JSON.stringify(value);
      localStorage.setItem(this.key, serializedValue);
    } catch (error) {
      console.error(`Error setting item in localStorage: ${error}`);
    }
  }

  remove(): void {
    try {
      localStorage.removeItem(this.key);
    } catch (error) {
      console.error(`Error removing item from localStorage: ${error}`);
    }
  }
}

export const storageService = {
  clientId: new StorageItem<number>('clientId'),
};


