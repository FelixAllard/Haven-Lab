async function sendEmailWithTemplateCall(directEmailModel) {
    const url = 'https://localhost:5158/gateway/api/ProxyEmailApi/sendwithformat'; // Replace with the actual URL

    const body = JSON.stringify(directEmailModel);

    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: body,
    });

    if (response.ok) {
        const data = await response.json();
        console.log('Email sent successfully:', data);
    } else {
        const errorData = await response.json();
        console.error('Error sending email:', errorData);
    }
}

// Example usage:
const directEmailModel = {
    EmailToSendTo: 'recipient@example.com',
    EmailTitle: 'Subject of the email',
    TemplateName: 'TemplateNameHere',
    Header: 'Header content here',
    Body: 'Email body content here',
    Footer: 'Footer content here',
    CorrespondantName: 'John Doe',
    SenderName: 'Jane Smith'
};

sendEmailWithTemplate(directEmailModel);
