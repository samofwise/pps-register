export const getFile = async (url: string) => {
  const response = await fetch(url);
  const blob = await response.blob();
  const fileName = getFileName(url);
  return new File([blob], fileName, { type: 'text/csv' });
};

export const getFileName = (url: string) => {
  const fileName = url.split('/').pop();
  if (!fileName) throw new Error('File name not found');
  return decodeURIComponent(fileName);
};