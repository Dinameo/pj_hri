import socket
import time

UDP_IP = "127.0.0.1"      # nếu Unity cùng máy
UDP_PORT = 5005

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

msg = "345,234,152,90,45,30"  # ví dụ: 6 góc quay của robot
while True:
    sock.sendto(msg.encode(), (UDP_IP, UDP_PORT))
    print(f"Sent: {msg}")
    time.sleep(1)  # gửi mỗi giây