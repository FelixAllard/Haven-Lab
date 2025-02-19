import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { motion } from 'motion/react';
import '../../Languages/i18n.js';
import { useTranslation } from 'react-i18next';
import HoverScaleWrapper from "../../Shared/HoverScaleWrapper";
import {FaFacebook, FaInstagram, FaLinkedin, FaTwitter} from "react-icons/fa";
import "./AboutUs.css";

// Data for team members
const teamMembers = [
  {
    name: 'John Doe',
    role: 'Co-Founder',
    imageLink:'https://www.goodfreephotos.com/albums/people/barber-giving-a-haircut.jpg',
    description:
      'John has been instrumental in the vision and direction of the company.',
    fullDescription:
      'John’s vision has shaped the company from the ground up. His leadership continues to guide the team to achieve our goals, while maintaining the company’s core values of innovation and teamwork.',
    facebookLink:"",
    twitterLink:"",
    instagramLink:"",
    linkedinLink:""
  },
  {
    name: 'Jane Smith',
    role: 'Co-Founder',
    imageLink:'https://th.bing.com/th/id/OIP.lSwGjCatQAV9OAtZZNrKUgHaFI?rs=1&pid=ImgDetMain',
    description:
      'Jane leads the strategy and operations with a focus on innovation and growth.',
    fullDescription:
      'With an eye for strategy, Jane has developed the blueprint for our growth and ensures our operations are aligned with the industry’s best practices. She is passionate about pushing the boundaries of what’s possible.',
    facebookLink:"",
    twitterLink:"",
    instagramLink:"",
    linkedinLink:""
  },
  {
    name: 'Emma Johnson',
    role: 'Creative Director',
    imageLink:'https://th.bing.com/th/id/OIP.TWP231UT_2YtiqYJST9MWwHaE8?rs=1&pid=ImgDetMain',
    description:
      'Emma brings creativity and design expertise to shape our brand identity.',
    fullDescription:
      'Emma’s designs have redefined our brand’s visual identity, ensuring that it resonates with our audience on a deeper level. She continuously explores new trends and adapts them to our work.',
    facebookLink:"",
    twitterLink:"",
    instagramLink:"",
    linkedinLink:""
  },
];

const AboutUs = () => {
  const [selectedMember, setSelectedMember] = useState(null);
  const [isExpanded, setIsExpanded] = useState(false);
  const { t } = useTranslation('aboutus');

  const handleLearnMoreClick = (member) => {
    setSelectedMember(member);
    setIsExpanded(true);
  };

  const handleCloseModal = () => {
    setIsExpanded(false);
    setSelectedMember(null);
  };

  return (
    <div className="mt-6">
      <Container className="py-5 bg-dark text-light">
        <Row className="mb-4">
          <Col>
            <motion.h1
              className="text-center"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1 }}
            >
              {t('About Us')}
            </motion.h1>
            <motion.p
              className="text-center"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1.5 }}
            >
              {t('Description')}
            </motion.p>
          </Col>
        </Row>

        <Row>
          {teamMembers.map((member, index) => (
              <HoverScaleWrapper>
                <Card className="bg-dark position-relative"> {/* Ensures overlay positioning */}
                  <Card.Img
                      src={member.imageLink}
                      className="card-img"
                      alt="Card Image"
                      style={{ height: "400px", objectFit: "cover" }} // Adjust height here
                  />
                  <Card.ImgOverlay className="d-flex flex-column justify-content-between h-100">
                    <Card.Title className="fs-3">
                      <b>{member.name}</b> - {member.role}
                    </Card.Title>
                    <Card.Text className="fs-4">
                      {member.fullDescription}
                    </Card.Text>
                    <Card.Text className="fs-5">
                      <motion.div
                          className="col-md-4"
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          transition={{
                            delay: 0.6,
                            duration: 0.3,
                            ease: "easeInOut",
                          }}
                      >
                        <h5>{t("Follow Me")}</h5>
                        <div className="social-media-icons">
                          <a href={member.facebookLink} target="_blank" rel="noopener noreferrer">
                            <FaFacebook />
                          </a>
                          <a href={member.twitterLink} target="_blank" rel="noopener noreferrer">
                            <FaTwitter />
                          </a>
                          <a href={member.instagramLink} target="_blank" rel="noopener noreferrer">
                            <FaInstagram />
                          </a>
                          <a href={member.linkedinLink} target="_blank" rel="noopener noreferrer">
                            <FaLinkedin />
                          </a>
                        </div>
                      </motion.div>
                    </Card.Text>
                  </Card.ImgOverlay>
                </Card>
              </HoverScaleWrapper>


          ))}
        </Row>

        {/* Modal-style Popup for "Learn More" */}
        {isExpanded && selectedMember && (
            <motion.div
                className="position-fixed top-0 left-0 w-100 h-100 bg-dark bg-opacity-75 d-flex justify-content-center align-items-center"
                initial={{opacity: 0}}
                animate={{opacity: 1}}
                transition={{duration: 0.5}}
            >
              <motion.div
                  className="bg-light text-dark p-4 rounded"
                  initial={{scale: 0}}
                  animate={{scale: 1}}
                  transition={{duration: 0.5}}
              >
                <h2>{selectedMember.name}</h2>
                <h4>{selectedMember.role}</h4>
                <p>{selectedMember.fullDescription}</p>
                <Button variant="dark" onClick={handleCloseModal}>
                  Close
                </Button>
              </motion.div>
            </motion.div>
        )}
      </Container>
    </div>
  );
};

export default AboutUs;
