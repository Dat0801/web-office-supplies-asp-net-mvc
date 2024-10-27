const cloudName = 'dvpzullxc';

async function uploadToCloudinary(file, folder) {
    const url = `https://api.cloudinary.com/v1_1/${cloudName}/image/upload`;
    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', folder);

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();
        if (result.secure_url) {
            return result.secure_url;
        } else {
            throw new Error('Image upload failed');
        }
    } catch (error) {
        console.error("Error uploading to Cloudinary:", error);
        throw error;
    }
}