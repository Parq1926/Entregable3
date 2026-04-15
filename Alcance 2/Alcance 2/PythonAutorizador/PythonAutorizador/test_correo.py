import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart

EMAIL_REMITENTE = "rojaspamela60@gmail.com"
EMAIL_PASSWORD = "bduixlpxkhfyttga"
EMAIL_DESTINATARIO = "rojaspamela60@gmail.com"

try:
    msg = MIMEMultipart()
    msg['From'] = EMAIL_REMITENTE
    msg['To'] = EMAIL_DESTINATARIO
    msg['Subject'] = "Prueba de correo"
    
    msg.attach(MIMEText("Este es un correo de prueba", 'plain', 'utf-8'))
    
    server = smtplib.SMTP('smtp.gmail.com', 587)
    server.starttls()
    server.login(EMAIL_REMITENTE, EMAIL_PASSWORD)
    server.send_message(msg)
    server.quit()
    
    print("✅ Correo enviado correctamente")
except Exception as e:
    print(f"❌ Error: {e}")